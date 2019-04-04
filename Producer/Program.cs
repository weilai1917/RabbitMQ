using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbitmqServer {
    class Program {
        static void Main (string[] args) {
            //1. 实例化连接工厂
            var factory = new ConnectionFactory ();
            factory.HostName = "localhost";
            factory.UserName = "admin";
            factory.Password = "admin";
            //2. 建立连接
            using (var connection = factory.CreateConnection ()) {
                //BaseRun(connection);
                ExhcangeFanout (connection);
            }
        }

        static void BaseRun (IConnection connection) {
            using (var channel = connection.CreateModel ()) {
                channel.QueueDeclare ("weilai", true, false, false, null);
                for (int i = 0; i < 10; i++) {
                    //5. 构建byte消息数据包
                    var body = Encoding.UTF8.GetBytes ($"Send {i}");
                    //6. 发送数据包
                    channel.BasicPublish ("", "weilai", null, body);
                }
            }
        }

        static void ExhcangeFanout (IConnection connection) {
            using (var channel = connection.CreateModel ()) {
                channel.ExchangeDeclare ("rabbitMQ", "fanout");
                //channel.QueueBind (channel.QueueDeclare ().QueueName, "rabbitMQ", "");
                //5. 构建byte消息数据包
                var body = Encoding.UTF8.GetBytes ($"Send 0");
                //6. 发送数据包
                channel.BasicPublish ("rabbitMQ", "asdf", null, body);
            }
        }
    }
}