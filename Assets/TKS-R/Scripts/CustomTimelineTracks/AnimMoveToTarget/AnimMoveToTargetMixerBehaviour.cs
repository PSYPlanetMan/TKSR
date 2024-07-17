using System;
using TKSR;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TKSRPlayables
{
	public class AnimMoveToTargetMixerBehaviour : PlayableBehaviour
	{
		private ICharacterSpriteRender trackBinding;
		private bool firstFrameHappened = false;

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			trackBinding = playerData as ICharacterSpriteRender;

			if (trackBinding == null)
				return;

			if (!firstFrameHappened)
			{
				//defaultPositions = trackBinding.GetCurrentPositions();
				firstFrameHappened = true;
			}

			//different behaviour depending if Unity is in Play mode or not,
			//because NavMeshAgent is not available in Edit mode
			if (Application.isPlaying)
			{
				ProcessPlayModeFrame(playable);
			}
		}


		//Happens in Play mode
		//Uses the NavMeshAgent to control the units, delegating their movement and animations to the AI
		private void ProcessPlayModeFrame(Playable playable)
		{
			int inputCount = playable.GetInputCount();

			for (int i = 0; i < inputCount; i++)
			{
				float inputWeight = playable.GetInputWeight(i);
				ScriptPlayable<AnimMoveToTargetBehaviour> inputPlayable =
					(ScriptPlayable<AnimMoveToTargetBehaviour>)playable.GetInput(i);
				AnimMoveToTargetBehaviour input = inputPlayable.GetBehaviour();

				//Make the Unit script execute the command
				if (inputWeight > 0f)
				{
					if (!input.commandExecuted)
					{
						FaceParam c = new FaceParam(input.faceType, input.targetPosition, input.targetTransform, input.animSpeed);
						trackBinding.MoveFaceToTarget(c);
						input.commandExecuted =
							true; //this prevents the command to be executed every frame of this clip
					}
				}
			}
		}

		public override void OnPlayableDestroy(Playable playable)
		{
			if (!Application.isPlaying)
			{
				firstFrameHappened = false;

				if (trackBinding == null)
					return;

				//trackBinding.SetPositions(defaultPositions);
			}
		}
	}
}