using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRSteelheaders_Server
{
    class Status
    {
        private int _status_id;
        private String _status_desc;

        public Status()
        {
            _status_id = 0;
            _status_desc = String.Empty;
        }

        public Status(int status_id, String status_desc)
        {
            ID = status_id;
            Description = status_desc;
        }

        public int ID
        {
            get
            {
                return _status_id;
            }

            set
            {
                if (value > 0)
                    _status_id = value;
            }
        }

        public String Description
        {
            get
            {
                return _status_desc;
            }

            set
            {
                if (value.Length > 0)
                    _status_desc = value;
            }
        }
    }
}
