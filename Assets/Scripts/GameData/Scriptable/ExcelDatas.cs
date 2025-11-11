using System;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(ExcelName = "MortalGames", AssetPath = "ScriptableObjects")]
public class ExcelDatas : ScriptableObject
{
	public List<ConstExcelData> Constant;
	public List<DispositionExcelData> Disposition;
	public List<LocalizeExcelData> LocalizeCard;
	public List<LocalizeExcelData> LocalizeCardBuff;
	public List<LocalizeExcelData> LocalizeKeyWord;
	public List<LocalizeExcelData> LocalizePlayerBuff;
	public List<LocalizeExcelData> LocalizePlayer;
}

[Serializable]

public class ConstExcelData
{
	public string Id;
	public string Value;
}

[Serializable]
public class DispositionExcelData
{
	public string Id;
	public int Range;
	public int RecoverEnergyPoint;
	public int DrawCardCount;
}

[Serializable]
public class LocalizeExcelData
{
	public string Id;
	public string Title;
	public string Info;
}