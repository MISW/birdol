using SQLite4Unity3d;

public class Character  {

	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }
	public string Name { get; set; }
	public int Rarity { get; set; }
	public int Visual { get; set; }
    public int Vocal { get; set; }
    public int Dance{ get; set; }

	public override string ToString ()
	{
		return string.Format ("[Character: Id={0}, Name={1},  Rarity={2}, Visual={3},  Vocal={4},  Dance={5}]", Id, Name, Rarity, Visual, Vocal, Dance);
	}
}