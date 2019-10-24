using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBank
{
    [Serializable]
	public class UserEntity {

		public string name;
        public string email;
        public string phone;
        public string score;
        public string register_datetime;
        public string is_submitted;
	}
}
