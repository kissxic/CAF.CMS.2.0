#### cwsharp.freq
���ĵĴʿ⣬������+��Ƶ�������޸Ĵ�Ƶ�ķ�ʽ���޸����մ������ϡ�

#### cwsharp.dic
��չ�ʵ��ļ���ÿ��һ�����顣

#### cwsharp.dawg
DAWG�ʵ䡣

#### �������µĴ��鵽�ֵ�
���µĴ�����ӵ�`cwsharp.dict`���Զ����ı��У�ͨ������ķ�ʽ��������DAWG�ļ���
```c#
 var rootPath = @"d:\"
var wordUtil = new WordUtil();
//����Ĭ�ϵĴ�Ƶ
using (var sr = new StreamReader(rootPath + @"\dict\cwsharp.freq", Encoding.UTF8))
{
	string line = null;
	while ((line = sr.ReadLine()) != null)
	{
		if (line == string.Empty) continue;
		var array = line.Split(' ');
		wordUtil.Add(array[0], int.Parse(array[1]));
	}            
}
//�����µĴʵ�
using (var sr = new StreamReader(rootPath + @"\dict\cwsharp.dic", Encoding.UTF8))
{
	string line = null;
	while ((line = sr.ReadLine()) != null)
	{
		if (line == string.Empty) continue;
		wordUtil.Add(line);
	}
}
//�����µ�dawg�ļ�
wordUtil.SaveTo(file);
```