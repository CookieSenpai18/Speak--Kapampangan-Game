using System;
using System.Collections.Generic;

[Serializable]
public class AntonymItem
{
    public int id;
    public string name;
    public string answer;
}

[Serializable]
public class AntonymDatabase
{
    public List<AntonymItem> items;
}