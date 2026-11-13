using System.Collections.Generic;

[System.Serializable]
public class SynonymItem
{
    public int id;
    public string name;
    public string answer;
}

[System.Serializable]
public class SynonymDatabase
{
    public List<SynonymItem> items;
}
