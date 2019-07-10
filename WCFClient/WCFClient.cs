using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;
using FormClient.ServerWCF;
//using FormClient.HelloWorldService;

namespace FormClient
{
    public partial class WCFClient : Form
    {
        static WCFClient form;

        public WCFClient()
        {
            InitializeComponent();

            form = this;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CommunicationState state = someProxy.State;
        }

        private void WCFClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            someProxy.StopService();
        }

        /// <summary>
        /// 호스트 로 부터 메시지를 받을 콜백 클래스
        /// </summary>>
        class ServerCallbackHandler : ServerWCF.IWCFTcpServerCallback
        {
            public void SetDataToClient(WCFMessageKind messageKind, object data)
            {
                form.Invoke(new MethodInvoker(delegate ()
                {
                    switch (messageKind)
                    {
                        case WCFMessageKind.Message:
                            form.textBox1.Text = data.ToString();
                            break;
                    }
                }));
            }
        }

        WCFTcpServerClient someProxy;

        private void button2_Click(object sender, EventArgs e)
        {
            NetTcpBinding bindig = new NetTcpBinding();
            bindig.Security.Mode = SecurityMode.None;
            bindig.ReliableSession.Enabled = false;

            //콜백 인스턴스를 구현한 클래스를 아래와 같이 적용한다;
            InstanceContext ict = new InstanceContext(new ServerCallbackHandler());

            /*SomeClient 는 서비스참조에 의해 자동으로 생성된 클래스로서
                DuplexClientBase<ISome> 을 상속하고
                ISome 을 구현 한다
                (정의로 이동) 하여 내용을 볼수 있다. */
            someProxy = new WCFTcpServerClient(ict);//
                //ict,
                //bindig,
                //new EndpointAddress("net.tcp://150.1.13.166/WCFTcpServer"));

            //호스트에 접속한다. 만약 호스트가 실행중이 아니라면 Exception..
            someProxy.Open();
            someProxy.StartService();
            return;

            //ServiceEndpoint ep =
            //    new ServiceEndpoint(ContractDescription.GetContract(typeof(WCFServerClient)),
            //    bindig,
            //    new EndpointAddress("net.tcp://150.1.13.166/WCFTcpServer"));

            //ChannelFactory<IWCFServer> factory = new ChannelFactory<IWCFServer>(ep);

            //IWCFServer proxy = factory.CreateChannel();
            //proxy.StartService();
            //(proxy as IDisposable).Dispose();
        }

        IHelloWorld helloProxy;

        private void button3_Click(object sender, EventArgs e)      
        {
            ServiceEndpoint ep = 
                new ServiceEndpoint(ContractDescription.GetContract(typeof(HelloWorldClient)), 
                new BasicHttpBinding(), 
                new EndpointAddress("http://150.1.13.166:1907/HelloService"));

            ChannelFactory<IHelloWorld> factory = new ChannelFactory<IHelloWorld>(ep);

            helloProxy = factory.CreateChannel();
            form.textBox1.Text = helloProxy.SayHello();
            (helloProxy as IDisposable).Dispose();
        }
    }
}
