using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;

class Program
{
	private static List<string> ParseString(string input, int index)
	{
		List<string> result = new List<string>();
		int start = 0;
		bool insideQuotes = false;
		for (int i = 0; i < input.Length; i++)
		{
			if (input[i] == '\"')
			{
				insideQuotes = !insideQuotes;
			}
			else if (input[i] == ',' && !insideQuotes)
			{
				result.Add(input.Substring(start, i - start));
				start = i + 1;
			}
		}
		result.Add(input.Substring(start));
		if (index == 1)
		{
			for (int i = 0; i < result.Count; i++)
			{
				result[i] = result[i].Replace("\"", "");
				result[i] = result[i].Replace("{", "");
				result[i] = result[i].Replace("}", "");
				result[i] = result[i].Trim();
			}
		}
		return result;
	}
	
	static void Main()
	{
		string filePath = "/Users/fgiuliano/Downloads/dataTestGrusso.csv";
		if (File.Exists(filePath))
		{
			List<string> answersString;
			string[] lines = File.ReadAllLines(filePath);
			var headers = new List<string>();
			var finalValues = new List<List<string>>();
			var dictionary = new List<Dictionary<string, string>>();
			bool isEmpty = false;
			if (lines.Length > 0)
			{
				string[] headersString = lines[0].Split(';');
				foreach (var str in headersString)
				{
					headers.Add(str);
				}
				int indexLimit = headers.IndexOf("answers_data");
				for (int i = 1; i < lines.Length; i++)
				{
					var values = new List<string>();
					string[] valuesString = lines[i].Split(';');
					int y = 0;
					dictionary.Add(new Dictionary<string, string>());
					foreach (var str in valuesString)
					{
						// if (headers[y] == "game_data")
						// {
						// 	dictionary[i - 1].Add(headers[y], ParseStringString(str, 1));
						// }
						// else
						// {
							if (y > indexLimit)
							{
								break;
							}
							dictionary[i - 1].Add(headers[y], str);
						//}
						values.Add(str);
						y++;
					}
					for (int j = 0; j < headers.Count && j < values.Count; j++)
					{
						var tempHeaders = new List<string>();
						if (headers[j] == "answers_data" && !string.IsNullOrEmpty(values[j]))
						{
							if (string.IsNullOrEmpty(values[j]))
							{
								isEmpty = true;
								break;
							}
							answersString = ParseString(values[j], 0);
							foreach (var str in answersString)
							{
								List<string> split = ParseString(str, 1);
								foreach (var variab in split)
								{
									string[] splitString = variab.Split('|');
									int index = 0;
									while (index < tempHeaders.Count)
									{
										index++;
									}

									if (index == tempHeaders.Count)
									{
										tempHeaders.Add(splitString[0]);
									}
									dictionary[i - 1].Add(splitString[0], splitString[1]);
								}
							}
						}

						foreach (var VARIABLE in tempHeaders)
						{
							int index = 0;
							while (index < headers.Count)
							{
								if (VARIABLE == headers[index])
								{
									break;
								}

								index++;
							}

							if (index == headers.Count)
							{
								headers.Add(VARIABLE);
							}
						}
					}
					if (isEmpty)
					{
						isEmpty = false;
					}
				}
				headers.Remove("answers_data");
				FinalTrim(headers, dictionary, finalValues);
				headers.Remove("Phone number");
				WriteToCsv(headers, finalValues);
			}
			else
			{
				Console.WriteLine("File is empty: " + filePath);
			}
		}
		else
		{
			Console.WriteLine("File not found: " + filePath);
		}
	}

	private static string ParseStringString(string input, int index)
	{
		List<string> result = new List<string>();
		int start = 0;
		bool insideQuotes = false;
		for (int i = 0; i < input.Length; i++)
		{
			if (input[i] == '\"')
			{
				insideQuotes = !insideQuotes;
			}
			else if (input[i] == ',' && !insideQuotes)
			{
				result.Add(input.Substring(start, i - start));
				start = i + 1;
			}
		}
		result.Add(input.Substring(start));
		if (index == 1)
		{
			for (int i = 0; i < result.Count; i++)
			{
				result[i] = result[i].Replace("\"", "");
				result[i] = result[i].Replace("{", "");
				result[i] = result[i].Replace("}", "");
				result[i] = result[i].Trim();
			}
		}
		string str = string.Join(',', result);
		return str;	
	}

	private static void FinalTrim(List<string> headers, List<Dictionary<string, string>> dictionary, List<List<string>> finalValues)
	{
		int i;
		int j = 0;
		foreach (var line in dictionary)
		{
			i = 0;
			finalValues.Add(new List<string>());
			while (i < headers.Count)
			{
				if (headers[i] == "phone" && line.ContainsKey("Phone number"))
				{
					string str;
					line.TryGetValue("Phone number", out str!);
					finalValues[j].Add(str);
					i++;
					continue;
				}
				if (line.ContainsKey(headers[i]) && headers[i] != "Phone number")
				{
					string str;
					line.TryGetValue(headers[i], out str!);
					finalValues[j].Add(str);
				}
				else if (headers[i] == "Phone number")
				{
					i++;
					continue;
				}
				else
				{
					finalValues[j].Add("");
				}

				i++;
			}

			j++;
		}
	}

	private static void WriteToCsv(List<string> headers, List<List<string>> values)
	{
		string newCsvPath = "/Users/fgiuliano/Downloads/sortedData.csv";
		string[] finalHeaders = headers.ToArray();
		using (StreamWriter writer = new StreamWriter(newCsvPath,false, Encoding.UTF8))
		{
			writer.WriteLine(string.Join(";", finalHeaders));
			foreach (var variable in values)
			{
				string[] finalValues = variable.ToArray();
				writer.WriteLine(string.Join(";", finalValues));
			}
		}
	}
}