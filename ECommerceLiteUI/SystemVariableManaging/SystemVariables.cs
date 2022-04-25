using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ECommerceLiteUI.SystemVariableManaging
{
    public static class SystemVariables
    {
        public static string EMAIL
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ECommerceLiteEmail"].ToString();
                }
                catch (Exception)
                {

                    throw new Exception("HATA: Webconfig dosyasında email bilgisi bulunamadı!");
                }



            }
        }




    }
}