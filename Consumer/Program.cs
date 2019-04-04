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
                //Hello Rabbit
                //BaseRun(connection);
                //Exchange-fanout
                ExhcangeFanout (connection);
            }
        }

        static void BaseRun (IConnection connection) {
            //3. 创建信道
            using (var channel = connection.CreateModel ()) {
                //4. 申明队列
                channel.QueueDeclare ("weilai", true, false, false, null);
                //5. 构造消费者实例
                var consumer = new EventingBasicConsumer (channel);
                //6. 绑定消息接收后的事件委托
                consumer.Received += (model, ea) => {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString (body);
                    Console.WriteLine ("已接收： {0}", message);
                    //channel.BasicReject(ea.DeliveryTag, false);
                    channel.BasicAck (ea.DeliveryTag, false);
                };
                //7. 启动消费者
                channel.BasicConsume ("weilai", false, consumer);
                Console.ReadLine ();
            }
        }
        static void ExhcangeFanout (IConnection connection) {
            //3. 创建信道
            using (var channel = connection.CreateModel ()) {
                //4. 申明Exchange
                channel.ExchangeDeclare ("rabbitMQ", "fanout");
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind (queueName, "rabbitMQ", "asdf");
                //5. 构造消费者实例
                var consumer = new EventingBasicConsumer (channel);
                //6. 绑定消息接收后的事件委托
                consumer.Received += (model, ea) => {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString (body);
                    Console.WriteLine ("已接收： {0}", message);
                };
                //7. 启动消费者
                channel.BasicConsume (queueName, true, consumer);
                Console.ReadLine ();
            }
        }
    }
}