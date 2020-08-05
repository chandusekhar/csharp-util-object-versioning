﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DAX.ObjectVersioning.Graph
{
    public class GraphNode : GraphObject, IGraphNode
    {
        // We need to use blocking collections, because a process (versioned transaction) can write to these collections while we read them
        private readonly BlockingCollection<GraphEdge> _inE = new BlockingCollection<GraphEdge>();
        private readonly BlockingCollection<GraphEdge> _outE = new BlockingCollection<GraphEdge>();

        public GraphNode(Guid id) : base(id)
        {

        }

        public IEnumerable<IGraphEdge> OutE(long version)
        {
            foreach (var edge in _outE)
            {
                if (edge.CreationVersion.InternalVersionId <= version && (edge.DeletionVersion == null || edge.DeletionVersion?.InternalVersionId > version))
                    yield return edge;
            }
        }

        public IEnumerable<IGraphEdge> InE(long version)
        {
            foreach (var edge in _inE)
            {
                if (edge.CreationVersion.InternalVersionId <= version && (edge.DeletionVersion == null || edge.DeletionVersion.InternalVersionId > version))
                    yield return edge;
            }
        }

        public IEnumerable<IGraphNode> Out(long version)
        {
            throw new NotImplementedException();
        }

        internal void AddIngoingEdge(GraphEdge edge)
        {
            _inE.Add(edge);
        }

        internal void AddOutgoingEdge(GraphEdge edge)
        {
            _outE.Add(edge);
        }
    }
}
