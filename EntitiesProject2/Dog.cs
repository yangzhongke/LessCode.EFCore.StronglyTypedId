using System;

namespace My.Name.Space
{
	[HasStronglyTypedId(typeof(int))]
	public class Dog
	{
		public DogId Id { get; set; }
		public string Name { get; set; }
	}
}

