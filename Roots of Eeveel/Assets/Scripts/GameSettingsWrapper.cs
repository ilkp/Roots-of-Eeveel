

[System.Serializable]
public class GameSettingsWrapper
{
	public int ResolutionX;
	public int ResolutionY;
	public int RefreshRate;
	public int FullscreenMode;
	public float Brightness;
	public float MusicVolume;
	public float SoundsVolume;

	public void setValues(int resolutionX, int resolutionY, int refreshRate, int fullscreenMode, float brightness, float musicVolume, float soundsVolume)
	{
		ResolutionX = resolutionX;
		ResolutionY = resolutionY;
		RefreshRate = refreshRate;
		FullscreenMode = fullscreenMode;
		Brightness = brightness;
		MusicVolume = musicVolume;
		SoundsVolume = soundsVolume;
	}
}
