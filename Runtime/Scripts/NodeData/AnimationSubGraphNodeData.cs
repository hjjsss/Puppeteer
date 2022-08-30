﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using GBG.Puppeteer.Graph;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;

namespace GBG.Puppeteer.NodeData
{
    [Serializable]
    public class AnimationSubGraphNodeData : AnimationNodeData
    {
        [SerializeField]
        private RuntimeAnimationGraph _subGraph;

        [SerializeField]
        private ParamBindingNameOrValue[] _paramBindingSources;


        public override AnimationNodeInstance CreateNodeInstance(PlayableGraph graph,
            Animator animator, Dictionary<string, AnimationNodeData> nodes,
            Dictionary<string, ParamInfo> parameters)
        {
            if (_paramBindingSources == null || _paramBindingSources.Length == 0)
            {
                return new AnimationSubGraphInstance(graph, animator, _subGraph, null);
            }

            var paramBindingSources = new ParamInfo[_subGraph.Parameters.Count];
            for (int i = 0; i < paramBindingSources.Length; i++)
            {
                foreach (var bindingSource in _paramBindingSources)
                {
                    if (bindingSource.TargetParamName.Equals(_subGraph.Parameters[i].Name))
                    {
                        paramBindingSources[i] = bindingSource
                            .GetParamBindingSource(parameters, _subGraph.Parameters[i].Type);
                        break;
                    }
                }
            }

            return new AnimationSubGraphInstance(graph, animator, _subGraph, paramBindingSources);
        }


        protected override AnimationNodeData InternalDeepClone()
        {
            var clone = new AnimationSubGraphNodeData()
            {
                _subGraph = this._subGraph,
            };

            clone._paramBindingSources = new ParamBindingNameOrValue[_paramBindingSources.Length];
            for (int i = 0; i < _paramBindingSources.Length; i++)
            {
                clone._paramBindingSources[i] = (ParamBindingNameOrValue)_paramBindingSources[i].Clone();
            }

            return clone;
        }
    }
}