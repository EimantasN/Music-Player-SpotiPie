using System;

namespace Models.BackEnd
{
    public class Quote
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime Created { get; set; }

        public Quote()
        {

        }

        public Quote(string quote)
        {
            this.Text = quote;
            Created = DateTime.Now;
        }
    }
}
