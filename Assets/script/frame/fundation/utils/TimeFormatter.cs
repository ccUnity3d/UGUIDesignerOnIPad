using UnityEngine;
using System.Collections;

/// <summary>
/// 时间格式化器（将“秒”转换为“小时:分钟:秒”）.
/// </summary>
public class TimeFormatter 
{
	public static string format(float secondNum) 
	{
		//小时.
		int hours = (int)secondNum / 60 / 60;
		
		//分钟.
		int minutes = (int)secondNum / 60 % 60;
		
		//秒.
		int seconds = (int)secondNum % 60;
		
		//毫秒.
		int fraction = (int)(secondNum * 1000) % 1000;
		
		string timeTxt = string.Format("{0:00}:{1:00}:{2:00}:{3:000}", hours, minutes, seconds, fraction); 
		
		return timeTxt;
	}
}
