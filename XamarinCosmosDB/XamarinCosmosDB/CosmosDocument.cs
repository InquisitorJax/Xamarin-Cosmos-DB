﻿using Newtonsoft.Json;
using System;

namespace XamarinCosmosDB
{
	public class CosmosDocument<T> where T : ModelBase
	{
		public CosmosDocument()
		{
		}

		public CosmosDocument(T model)
		{
			Id = model.Id;
			Model = model;
			Type = typeof(T).Name;
			UserId = App.CurrentUserId; //user Id is used as the partition key
		}

		/// <summary>
		/// Will be the same as the Model.Id
		/// </summary>
		[JsonProperty("id")] //NOTE: Must be lower-case to match the "id" property defined in CosmosDB Container
		public string Id { get; set; }

		public T Model { get; set; }

		public string Type { get; set; }

		/// <summary>
		/// User Id is the partition key!
		/// </summary>
		public string UserId { get; set; }
	}

	public class ModelBase
	{

		public ModelBase()
		{
			Id = Guid.NewGuid().ToString();
		}

		public string Id { get; set; }
	}

	public class TestModel : ModelBase
	{
		public string Name { get; set; }
	}

	public class TestModel2 : TestModel
	{
		private DateTime _date;

		public int Number { get; set; }

		// always store dates as UTC
		// doc: https://docs.microsoft.com/en-us/azure/cosmos-db/working-with-dates

		public DateTime Date //{ get; set; }
		{ 
			get => _date; 
			set => _date = value.ToUniversalTime(); 
		}
	}
}
