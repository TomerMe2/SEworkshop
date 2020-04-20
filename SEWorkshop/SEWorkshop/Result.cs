using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    public class Result
    {
        bool Successful { get; set; }
        public string Message { get; set; }

        protected Result(string Message, bool isSuccessful)
        {
            this.Successful = false;
            this.Message = Message;
        }

        public bool isSuccessful()
        {
            return Successful;
        }

        public static Result Success(string message)
        {
            return new Result(message, true);
        }

        public static Result Error(string errorMessage)
        {
            return new Result(errorMessage, false);
        }
    }
}
