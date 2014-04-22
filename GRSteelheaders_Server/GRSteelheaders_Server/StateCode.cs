using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRSteelheaders_Server
{
    public class StateCode
    {
        private String _state_cd;
        private String _state_nm;

        public StateCode()
        {
            _state_cd = String.Empty;
            _state_nm = String.Empty;
        }

        public StateCode(String state_cd, String state_nm)
        {
            _state_cd = state_cd;
            _state_nm = state_nm;
        }

        public String Code
        {
            get
            {
                return _state_cd;
            }
        }

        public String Name
        {
            get
            {
                return _state_nm;
            }
        }


    }
}
