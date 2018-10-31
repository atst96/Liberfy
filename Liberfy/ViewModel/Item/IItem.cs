namespace Liberfy
{
	[System.Flags]
	enum ItemType : int
	{
		Status					= 0x001,
		Activity				= 0x002,
		User					= 0x004,
		Message					= 0x008,
		Retweet					= Status | 0x010,
		FavoriteActivity		= Activity | 0x020,
		RetweetActivity			= Activity | 0x040,
		FollowActivity			= Activity | 0x080,
		ListMemberAddedActivity	= Activity | 0x100,
		FavoritedRetweet		= Activity | 0x200,
		RetweetedRetweet		= Activity | 0x400
	}

	interface IItem
	{
		ItemType Type { get; }
		bool IsAnimatable { get; }
	}
}
