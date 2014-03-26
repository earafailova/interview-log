using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CsvHelper;
using interview_log.Models;
using CsvHelper.Configuration;
using interview_log.Helpers;


namespace interview_log
{
    public class UserMap : CsvClassMap<User>
    {
          public override void CreateMap()
         {
             Map(m => m.Id).Name("Id");
             Map(m => m.UserName).Name("UserName"); 
             Map(m => m.Email).Name("Email"); 
             Map(m => m.State).Name("State"); 
             Map(m => m.Position).Name("Position");
             Map(m => m.Tags).TypeConverter(new TagsConverter()).Name("Tags");
         }

          private User user(ICsvReaderRow arg)
          {
              throw new NotImplementedException();
          }
    }

  
}