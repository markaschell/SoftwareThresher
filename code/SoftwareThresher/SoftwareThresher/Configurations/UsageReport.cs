using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SoftwareThresher.Utilities;
using Console = SoftwareThresher.Utilities.Console;

namespace SoftwareThresher.Configurations {
   public class UsageReport {
      const string Indent = "\t";

      readonly IConsole console;
      readonly IAssemblyObjectFinder assemblyObjectFinder;

      public UsageReport() : this(new AssemblyObjectFinder(), new Console()) { }

      public UsageReport(IAssemblyObjectFinder assemblyObjectFinder, IConsole console) {
         this.console = console;
         this.assemblyObjectFinder = assemblyObjectFinder;
      }

      public void Write() {
         console.WriteLine("Usage: SoftwareThresher.exe config.xml [config.xml]");

         WriteSettings();
         console.WriteLine("");
         WriteTasks();
      }

      void WriteSettings() {
         var settings = assemblyObjectFinder.SettingTypes.ToList();

         foreach (var settingInterface in InAlphabeticalOrder(assemblyObjectFinder.SettingInterfaces)) {
            WriteSettingType(settingInterface, settings);
         }
      }

      static IEnumerable<Type> InAlphabeticalOrder(IEnumerable<Type> types) {
         return types.OrderBy(t => t.Name);
      }

      void WriteSettingType(Type settingInterface, IEnumerable<Type> settings) {
         console.WriteLine($"{Indent}Setting Type:{Indent}{settingInterface.Name}");

         foreach (var setting in InAlphabeticalOrder(settings.Where(s => s.GetInterfaces().Contains(settingInterface)))) {
            WriteSetting(setting);
         }
      }

      void WriteSetting(Type setting) {
         console.WriteLine($"{Indent}{Indent}Setting:{Indent}{setting.Name}{GetUsageNote(setting)}");
         WriteProperties(setting, Indent);
      }

      static string GetUsageNote(MemberInfo objectToCheck) {
         var noteAttribute = (UsageNoteAttribute)GetCustomAttribute(objectToCheck, typeof(UsageNoteAttribute));
         return noteAttribute != null ? " - " + noteAttribute.Note : string.Empty;
      }

      static Attribute GetCustomAttribute(MemberInfo objectToCheck, Type attributeType) {
         return Attribute.GetCustomAttributes(objectToCheck).FirstOrDefault(a => a.GetType() == attributeType);
      }

      void WriteProperties(Type type, string prefix = "") {
         var properties = GetProperties(type);

         var requiredProperties = new List<PropertyInfo>();
         var optionalProperties = new List<PropertyInfo>();

         foreach (var property in properties) {
            var optionalAttribute = (OptionalAttribute)GetCustomAttribute(property, typeof(OptionalAttribute));

            if (optionalAttribute != null) {
               optionalProperties.Add(property);
            }
            else {
               requiredProperties.Add(property);
            }
         }

         foreach (var property in requiredProperties) {
            WriteProperty(property, false, prefix);
         }

         foreach (var property in optionalProperties) {
            WriteProperty(property, true, prefix);
         }
      }

      static IEnumerable<PropertyInfo> GetProperties(Type type) {
         return type.GetProperties().Where(a => a.CanWrite).OrderBy(p => p.Name);
      }

      void WriteProperty(PropertyInfo property, bool isOptional, string prefix) {
         var optionalText = isOptional ? "Optional " : string.Empty;
         var propertyIndent = isOptional ? Indent : $"{Indent}{Indent}";

         console.WriteLine($"{prefix}{Indent}{Indent}{optionalText}Attribute:{propertyIndent}{property.Name} ({property.PropertyType.Name}){GetUsageNote(property)}");
      }

      void WriteTasks() {
         foreach (var task in InAlphabeticalOrder(assemblyObjectFinder.TaskTypes)) {
            console.WriteLine($"{Indent}Task:{Indent}{task.Name}({GetParameterText(task)}){GetUsageNote(task)}");
            WriteProperties(task);
         }
      }

      static string GetParameterText(Type task) {
         var parameters = task.GetConstructors().Single().GetParameters().OrderBy(p => p.Name);
         return string.Join(", ", parameters.Select(p => p.ParameterType.Name));
      }
   }
}
