using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DOCVIDEO.Domain;
using System.Collections.Generic;
using CompanyDatabaseInitialization;
namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void InitializeDB()
        {
            using (CompanyDatabase languageContext = new CompanyDatabase())
            {
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "EN", DESCRIPTION="English", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "AR", DESCRIPTION="Arabic", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });                
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "FR", DESCRIPTION="French", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "DE", DESCRIPTION="German", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "EL", DESCRIPTION="Greek", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "HI", DESCRIPTION="Hindi", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "IT", DESCRIPTION="Italian", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "JA", DESCRIPTION="Japanese", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "KO", DESCRIPTION="Korean", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "ZH", DESCRIPTION="Mandarin", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "PL", DESCRIPTION="Polish", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "PT", DESCRIPTION="Portuguese", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "RO", DESCRIPTION="Romanian", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "RU", DESCRIPTION="Russian", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "ES", DESCRIPTION="Spanish", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "TR", DESCRIPTION="Turkish", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.LANGUAGES.Add(new LANGUAGE() { LANGUAGEKEYID = "VI", DESCRIPTION="Vietnamese", CREATEDBY = "Anoop", MODIFIEDBY = "Anoop", DateCreated = DateTime.Now, MODIFIEDON = DateTime.Now });
                languageContext.SaveChanges();
            }
           

          
        }
    }
}
