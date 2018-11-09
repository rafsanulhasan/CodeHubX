using CodeHubX.Helpers;
using System;
using System.Collections.Generic;

namespace CodeHubX.Models
{
	public partial class BackgroundTaskBuilderModel
	{
		protected ICollection<object> _Conditions;

		public string Name { get; private set; }
		public object Trigger { get; private set; }

		public Type EntryPointType { get; private set; }
		public string Group { get; private set; }

		private BackgroundTaskBuilderModel(string name) => SetName(name);

		public BackgroundTaskBuilderModel(string name, object trigger)
		    : this(name) => SetTrigger(trigger);

		public BackgroundTaskBuilderModel(string name, object trigger, Type entryPointType = null, string group = null, params object[] conditions)
		    : this(name)
		{
			SetTrigger(trigger);
			if (entryPointType != null)
				SetEntryPointType(entryPointType);
			if (!StringHelper.IsNullOrEmptyOrWhiteSpace(group))
				SetGroup(group);
			if (conditions != null && conditions.Length > 0)
				SetConditions(conditions);

		}

		public void SetName(string name)
			=> Name = StringHelper.IsNullOrEmptyOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : name;

		partial void SetTrigger(object trigger);

		partial void AddCondition(object condition);

		partial void AddConditions(params object[] conditions);

		public virtual ref readonly ICollection<object> GetConditions()
			=> ref _Conditions;

		partial void CombineConditions(params object[] conditions);

		partial void RemoveConditions(params object[] conditions);

		partial void SetConditions(params object[] conditions);

		public void SetGroup(string groupName)
			=> Group = groupName;

		public void SetEntryPointType(Type entryPointType)
			=> entryPointType = entryPointType ?? throw new ArgumentNullException(nameof(entryPointType));
	}
}
