namespace DataBank
{
    [System.Serializable]
    public class UniversalUserEntity
    {
        // primary data
        public string id;
        public string name;
        public string email;
        public string contact;
        public string game_score;
        public string register_datetime;
        public string is_submitted = "false";
        /*  public string age;
          public string dob;
          public string gender;
          public string game_result;
          public string voucher_id;
          
          */
    }
}
