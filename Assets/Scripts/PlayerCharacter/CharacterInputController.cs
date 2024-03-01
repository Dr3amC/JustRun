using UnityEngine;

namespace PlayerCharacter
{
	// Инпут контроллер, для редатора и тач девайсов
    public class CharacterInputController : MonoBehaviour
    {
	    [SerializeField] private Character _character;

#if !UNITY_EDITOR
		private Vector2 _startingTouch;
		private bool _isSwiping;
#endif
	    
        private void Update()
        {
#if UNITY_EDITOR
	        if (Input.GetKeyDown(KeyCode.LeftArrow))
	        {
		        _character.CharacterMovementController.ChangeLane(-1);
	        }
	        else if(Input.GetKeyDown(KeyCode.RightArrow))
	        {
		        _character.CharacterMovementController.ChangeLane(1);
	        }
	        else if(Input.GetKeyDown(KeyCode.UpArrow))
	        {
		        _character.CharacterMovementController.Jump();
	        }
#else

	        if (Input.touchCount == 1)
	        {
				if(_isSwiping)
				{
					var diff = Input.GetTouch(0).position - _startingTouch;
					diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);

					if (diff.magnitude > 0.01f)
					{
						if (Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
						{
							if (diff.y > 0)
							{
								_character.CharacterMovementController.Jump();
							}
						}
						else
						{
							if (diff.x < 0)
							{
								_character.CharacterMovementController.ChangeLane(-1);
							}
							else
							{
								_character.CharacterMovementController.ChangeLane(1);
							}
						}
							
						_isSwiping = false;
					}
	            }

				if (Input.GetTouch(0).phase == TouchPhase.Began)
				{
					_startingTouch = Input.GetTouch(0).position;
					_isSwiping = true;
				}
				else if (Input.GetTouch(0).phase == TouchPhase.Ended)
				{
					_isSwiping = false;
				}
	        }
#endif
        }
    }
}
