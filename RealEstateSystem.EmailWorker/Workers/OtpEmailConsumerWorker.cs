using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RealEstateSystem.Application.DTOs.Mail;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.EmailWorker.Workers
{
    public class OtpEmailConsumerWorker : BackgroundService
    {
        private readonly IConnection _rabbitMqConnection;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OtpEmailConsumerWorker> _logger;

        public OtpEmailConsumerWorker(
            IConnection rabbitMqConnection,
            IMailService mailService,
            IConfiguration configuration,
            ILogger<OtpEmailConsumerWorker> logger)
        {
            _rabbitMqConnection = rabbitMqConnection;
            _mailService = mailService;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueName = _configuration["RabbitMq:OtpEmailQueue"] ?? "otp-email-queue";

            await using var channel = await _rabbitMqConnection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                var jsonPayload = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                SendOtpEmailMessage? emailMessage = null;

                try
                {
                    emailMessage = JsonSerializer.Deserialize<SendOtpEmailMessage>(jsonPayload);
                }
                catch (JsonException)
                {
                    _logger.LogError("Không thể deserialize tin nhắn từ queue. Payload: {Payload}", jsonPayload);
                    await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                    return;
                }

                if (emailMessage == null)
                {
                    await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                    return;
                }

                try
                {
                    var subject = "Mã xác thực OTP - Real Estate System";
                    var body = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto;'>
                            <h2>Xin chào {emailMessage.FullName},</h2>
                            <p>Mã OTP đăng ký tài khoản của bạn là:</p>
                            <h1 style='letter-spacing: 8px; color: #2563eb;'>{emailMessage.OtpCode}</h1>
                            <p>Mã này có hiệu lực trong <strong>5 phút</strong>. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
                            <p style='color: #6b7280;'>Real Estate System</p>
                        </div>";

                    await _mailService.SendEmailAsync(emailMessage.ToEmail, subject, body, stoppingToken);
                    await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi gửi email OTP tới {Email}. Đang đưa lại vào hàng đợi.", emailMessage.ToEmail);
                    await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
