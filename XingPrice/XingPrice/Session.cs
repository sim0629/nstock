using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XA_SESSIONLib;

namespace XingPrice
{
    class Session
    {
        private IXASession session_body;
        private _IXASessionEvents_Event session_event;

        public bool IsConnected
        {
            get
            {
                return session_body != null && session_body.IsConnected();
            }
        }

        public Session(_IXASessionEvents_LoginEventHandler login_handler)
        {
            var session = (XASession)Activator.CreateInstance(Type.GetTypeFromProgID("XA_Session.XASession"));
            session_body = session;
            session_event = session;
            session_event.Login += login_handler;
        }
        
        public bool Connect()
        {
            if (!session_body.ConnectServer("hts.ebestsec.co.kr", 20001))
            {
                Console.WriteLine("Fail to connect");
                return false;
            }
            Console.WriteLine("Success to connect");
            return true;
        }

        public void Login(string id, string pass, string cert_pass)
        {
            session_body.Login(id, pass, cert_pass, 0, true);
        }

        public void Disconnect()
        {
            if (IsConnected) session_body.DisconnectServer();
            session_body = null;
            session_event = null;
        }
    }
}
