using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SoftwareThresher.Tasks;
using SoftwareThresher.Settings;

namespace SoftwareThresher.Configurations
{
   public interface IAssemblyObjectFinder
   {
      IEnumerable<Type> TaskTypes { get; }
      IEnumerable<Type> SettingTypes { get; }
      IEnumerable<Type> SettingInterfaces { get; }
   }

   public class AssemblyObjectFinder : IAssemblyObjectFinder
   {
      readonly Type settingBaseInterface = typeof(Setting);

      public IEnumerable<Type> TaskTypes => GetTypes.Where(t => t.IsClass && t.BaseType == typeof(Task));

      public IEnumerable<Type> SettingTypes => GetTypes.Where(t => t.IsClass && t.GetInterfaces().Contains(settingBaseInterface));

      public IEnumerable<Type> SettingInterfaces => GetTypes.Where(t => t.IsInterface && t.GetInterfaces().Contains(settingBaseInterface));

      static IEnumerable<Type> types;
      static IEnumerable<Type> GetTypes => types ?? (types = Assembly.GetExecutingAssembly().GetTypes());
   }
}
