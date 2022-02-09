using LevelGen;

namespace Game.UI
{
	public class LoadLevel : UIElement
	{
		public UnityEngine.UI.Slider _slider;
		public UnityEngine.UI.Text _createStep;
		private LevelBuilder _builder;

		private void Start()
		{
			enabled = false;
		}

		private void Update()
		{
			_slider.value = _builder.Progress();
			_createStep.text = _builder.ProgressStep();
		}

		public void SetBuilder(LevelBuilder builder)
		{
			enabled = builder != null;
			_builder = builder;
		}
	}
}
