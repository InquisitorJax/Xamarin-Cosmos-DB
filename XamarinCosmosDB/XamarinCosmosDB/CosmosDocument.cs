using System;
using System.Collections.Generic;
using System.Text;

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
			UserId = App.CurrentUserId; //user Id is used as the partition key
		}

		/// <summary>
		/// Will be the same as the Model.Id
		/// </summary>
		public string Id { get; set; }

		public T Model { get; set; }

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
}
