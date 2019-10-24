using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBank
{
    [Serializable]
	public class UserEntity {

        // primary data
		public string id;
		public string name;
        public string email;
        public string contact;
        public string age;
        public string dob;
        public string gender;
        public string game_result;
        public string game_score;
        public string voucher_id;
        public string register_datetime;
        public string is_submitted;
	}
}

  /**
     * Player data submission API
     * 
     * 2 possible results response from this player data submission api:
     * 1. Success
     * 2. Fail
     * 
     * Requires 8 parameters
     * 1. name
     * 2. email
     * 3. phone
     * 4. age
     * 5. dob
     * 6. gender
     * 7. game_result
     * 8. game_score
     * 9. voucher_name
     * 10. register_datetime
     */