using DKP.Core.Infrastructure;

namespace DKP.Core.Models
{
    public class PageParam
    {
        public int limit { get; set; }
        public int offset { get; set; }
        public string sort { get; set; }
        public string order { get; set; }

        public PageParam()
        {
            this.limit = 10;
            this.offset = 0;
            this.sort = "id";
            this.order = "desc";
        }
        public override string ToString() { return sort + " " + order; }
    }
}
