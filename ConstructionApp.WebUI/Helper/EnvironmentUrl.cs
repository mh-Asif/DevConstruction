namespace ConstructionApp.WebUI.Helper
{
    public static class EnvironmentUrl
    {
      public static string webApp = "https://localhost:7128";   
	  //public static string webApp = "https://cms.hrclicks.com";
	   public static string apiUrl = "https://localhost:7013/api";	
	//public static string apiUrl = "https://cmsapi.hrclicks.com/api";
		public static string apiUserUrl = apiUrl + "/UsersAPI/";
        public static string apiMasterUrl = apiUrl + "/MasterAPIs/";
        public static string apiProjectUrl = apiUrl + "/ProjectAPI/";
        public static string apiTaskUrl = apiUrl + "/TaskAPI/";
        public static string apiCommentsUrl = apiUrl + "/UserCommentsAPI/";
        public static string apiSubTaskUrl = apiUrl + "/SubTaskAPI/";
        public static string apiProjectFileUrl = apiUrl + "/ProjectFilesAPI/";
        public static string apiPhotoFileUrl = apiUrl + "/ProjectPhotoAPI/";

    }
}
