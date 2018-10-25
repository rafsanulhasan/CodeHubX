using CodeHubX.Helpers;
using CodeHubX.UWP.Helpers;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;

namespace CodeHubX.UWP.Models
{
	public class BackgroundTaskBuilderModel
	{
		private ICollection<IBackgroundCondition> _Conditions;

		public string Name { get; private set; }
		public IBackgroundTrigger Trigger { get; private set; }

		public Type EntryPointType { get; private set; }
		public string Group { get; private set; }

		private BackgroundTaskBuilderModel(string name) => SetName(name);

		public BackgroundTaskBuilderModel(string name, IBackgroundTrigger trigger)
		    : this(name) => SetTrigger(trigger);

		public BackgroundTaskBuilderModel(string name, IBackgroundTrigger trigger, Type entryPointType = null, string group = null, params IBackgroundCondition[] conditions)
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

		public void SetTrigger(IBackgroundTrigger trigger) 
			=> Trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));

		public void AddCondition(IBackgroundCondition condition)
		{
			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			_Conditions = _Conditions ?? new UniqueCollection<IBackgroundCondition>();

			if (!_Conditions.Contains(condition))
			{
				_Conditions.Add(condition);
			}
		}

		public void AddConditions(params IBackgroundCondition[] conditions)
		{
			if (conditions == null)
			{
				throw new ArgumentNullException(nameof(conditions));
			}

			_Conditions = _Conditions ?? new UniqueCollection<IBackgroundCondition>();

			foreach (var condition in conditions)
			{
				if (!_Conditions.Contains(condition))
				{
					_Conditions.Add(condition);
				}
			}
		}

		public ref readonly ICollection<IBackgroundCondition> GetConditions() 
			=> ref _Conditions;

		public void CombineConditions(params IBackgroundCondition[] conditions) 
			=> _Conditions = _Conditions.Combine(conditions);

		public void RemoveConditions(params IBackgroundCondition[] conditions)
		{
			if (conditions == null)
			{
				throw new ArgumentNullException(nameof(conditions));
			}

			_Conditions = _Conditions ?? new UniqueCollection<IBackgroundCondition>();

			foreach (var condition in conditions)
			{
				if (_Conditions.Contains(condition))
				{
					_Conditions.Remove(condition);
				}
			}
		}

		public void SetConditions(params IBackgroundCondition[] conditions) 
			=> _Conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));

		public void SetGroup(string groupName) 
			=> Group = groupName;

		public void SetEntryPointType(Type entryPointType) 
			=> entryPointType = entryPointType ?? throw new ArgumentNullException(nameof(entryPointType));
	}
}
