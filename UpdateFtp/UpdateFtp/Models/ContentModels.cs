using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpdateFtp.Models
{
    public class ContentModels
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string link;

        public string Link
        {
            get { return link; }
            set { link = value; }
        }
        private string size;

        public string Size
        {
            get { return size; }
            set { size = value; }
        }
        private string time;
        public string Time
        {
            get { return time; }
            set { time = value; }
        }
    }
}