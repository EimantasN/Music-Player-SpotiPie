using System;
using System.Collections.Generic;
using System.Text;

namespace Models.BackEnd
{
    public class Error
    {
        public int Id { get; set; }
        public string Message { get; set; }

        public string Method { get; set; }

        public DateTime Created { get; set; }

        public Error()
        {

        }

        public Error(string msg, string method)
        {
            this.Method = method;
            this.Message = msg;
            this.Created = DateTime.Now;
        }
    }
}
