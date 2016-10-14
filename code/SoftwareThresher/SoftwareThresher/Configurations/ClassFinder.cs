using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SoftwareThresher.Tasks;
using SoftwareThresher.Settings;

namespace SoftwareThresher.Configurations {
   public interface IClassFinder
   {
      IEnumerable<Type> TaskTypes { get; }
      IEnumerable<Type> SettingTypes { get; }
   }

   public class ClassFinder : IClassFinder
   {
      public IEnumerable<Type> TaskTypes => Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.GetInterfaces().Contains(typeof(Task)));
      public IEnumerable<Type> SettingTypes => Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.GetInterfaces().Contains(typeof(Setting)));
   }
}
