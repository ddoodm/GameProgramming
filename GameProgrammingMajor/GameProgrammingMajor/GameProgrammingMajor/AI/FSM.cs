using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GameProgrammingMajor
{
    public enum Condition
    {
        atTarget,
        lowHealth,
        awayFromAttacker,
        noAmmo,
    }

    public enum StateName
    {
        gotoTarget,
        attackTarget,
        evadeAttacker,
        destroy,
    }

    public class Transition
    {
        private NPC npc;
        public string actionName { get; protected set; }
        public Condition condition { get; protected set; }
        public State action { get; protected set; }

        public Transition(Condition condition, string actionName)
        {
            this.condition = condition;
            this.actionName = actionName;
        }

        public void setNpc(NPC npc)
        {
            this.npc = npc;
        }

        public void consolidateActionState(State action)
        {
            this.action = action;
        }

        public bool conditionIsTrue()
        {
            return npc.tryEvaluateCondition(condition);
        }
    }

    public class State
    {
        private NPC npc;
        public StateName name { get; protected set; }
        public List<Transition> transitions { get; protected set; }

        public State(StateName name)
        {
            this.name = name;

            transitions = new List<Transition>();
        }

        public void setNpc(NPC npc)
        {
            this.npc = npc;
            foreach (Transition transition in transitions)
                transition.setNpc(npc);
        }

        public void addTransition(Transition transition)
        {
            transitions.Add(transition);
        }

        public string stateNameAsString()
        {
            return Enum.GetName(typeof(StateName), name);
        }

        public State getNextState()
        {
            // Transition to the first condition that is met
            foreach (Transition transition in transitions)
            {
                if (transition.conditionIsTrue())
                    return transition.action;
            }

            // Otherwise, continue with this state
            return this;
        }

        public void execute(UpdateParams updateParams)
        {
            npc.executeState(updateParams, name);
        }
    }

    /// <summary>
    /// Finite State Machine whose data is obtained from an XML document.
    /// </summary>
    public class FSM
    {
        private List<State> states;
        private State currentState;
        private NPC npc;

        public FSM(NPC npc, string fsmFile)
        {
            this.npc = npc;
            states = FSMLoader.loadStates(fsmFile);

            consolidateStates();

            currentState = states[0];
        }

        private void consolidateStates()
        {
            foreach (State state in states)
            {
                // Give state NPC
                state.setNpc(npc);

                // Consolidate transition action states
                foreach (Transition transition in state.transitions)
                {
                    State action = findStateWithName(transition.actionName);
                    transition.consolidateActionState(action);
                }
            }
        }

        private State findStateWithName(string nameString)
        {
            foreach (State state in states)
                if (state.stateNameAsString().Equals(nameString))
                    return state;
            return null;
        }

        public void update(UpdateParams updateParams)
        {
            currentState = currentState.getNextState();

            currentState.execute(updateParams);
        }
    }

    public static class FSMLoader
    {
        /// <summary>
        /// Load states from an XML file
        /// </summary>
        /// <param name="fsmFile">The XML file path</param>
        /// <returns>The states declared in the XML file</returns>
        public static List<State> loadStates(string fsmFile)
        {
            XmlReader xml = XmlReader.Create(fsmFile);
            List<State> states = new List<State>();

            while (xml.Read())
            {
                if (xml.NodeType == XmlNodeType.Element)
                    if (xml.Name == "states")
                        states = readStates(xml, states);
            }

            return states;
        }

        private static List<State> readStates(XmlReader xml, List<State> states)
        {
            while (xml.Read())
            {
                // Read until the tag ends
                if (xml.NodeType == XmlNodeType.EndElement &&
                    xml.Name.Equals("states", StringComparison.OrdinalIgnoreCase))
                    break;

                if (xml.Name.Equals("state"))
                    states.Add(readState(xml));
            }

            return states;
        }

        private static State readState(XmlReader xml)
        {
            State state = new State(stringToStateName(xml["name"]));

            while (xml.Read())
            {
                // Read until the tag ends
                if (xml.NodeType == XmlNodeType.EndElement &&
                    xml.Name.Equals("state", StringComparison.OrdinalIgnoreCase))
                    break;

                if (xml.Name.Equals("transition"))
                    state.addTransition(readTransition(xml));
            }

            return state;
        }

        private static StateName stringToStateName(string stateString)
        {
            foreach (StateName stateName in Enum.GetValues(typeof(StateName)))
                if (stateString.Equals(Enum.GetName(typeof(StateName), stateName)))
                    return stateName;

            throw new Exception("An unknown state \"" + stateString + "\" was encountered.");
        }

        private static Condition stringToCondition(string conditionString)
        {
            foreach (Condition condition in Enum.GetValues(typeof(Condition)))
                if (conditionString.Equals(Enum.GetName(typeof(Condition), condition)))
                    return condition;

            throw new Exception("An unknown condition \"" + conditionString + "\" was encountered.");
        }

        private static Transition readTransition(XmlReader xml)
        {
            string conditionName = xml["condition"];
            string actionName = xml["action"];
            return new Transition(stringToCondition(conditionName), actionName);
        }
    }
}
