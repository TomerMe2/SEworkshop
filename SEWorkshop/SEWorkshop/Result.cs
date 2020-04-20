using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Result
    {
        bool Successful { get; set; }
        public string ErrorMessage { get; set; }

        protected Result()
        {
            this.Successful = true;
            this.ErrorMessage = "";
        }

        protected Result(String ErrorMessage)
        {
            this.Successful = false;
            this.ErrorMessage = ErrorMessage;
        }

        public bool isSuccessful()
        {
            return Successful;
        }

        public static Result Success()
        {
            return new Result();
        }

        public static Result Error(String errorMessage)
        {
            return new Result(errorMessage);
        }
    }
}
