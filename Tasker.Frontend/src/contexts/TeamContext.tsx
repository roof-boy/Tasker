import {
  createContext,
  useState,
  useContext,
  type ReactNode,
  useEffect,
} from "react";
import type { Team } from "../types/Team";
import { GetTeamsForUser } from "../api/teamActions";
import { useAuth } from "./AuthContext";

interface TeamContextType {
  teams: Team[] | null;
  selectedTeam: Team | null;
  selectTeam: (team: Team) => void;
  refreshTeams: () => void;
  teamsLoading: boolean;
}

const TeamContext = createContext<TeamContextType>({
  teams: null,
  selectedTeam: null,
  selectTeam: () => console.warn("selectTeam called without provider"),
  refreshTeams: () => console.warn("refreshTeams called without provider"),
  teamsLoading: true,
});

export const TeamProvider = ({ children }: { children: ReactNode }) => {
  const [teams, setTeams] = useState<Team[] | null>(null);
  const [selectedTeam, setSelectedTeam] = useState<Team | null>(null);
  const [teamsLoading, setTeamsLoading] = useState<boolean>(true);

  const { user, isLoading } = useAuth();

  async function refreshTeams() {
    if (user && !isLoading) {
      try {
        setTeamsLoading(true);
        const response = await GetTeamsForUser();

        if (response.success) {
          setTeams(response.data);
        }
      } catch (error) {
        console.error(error);
      } finally {
        setTeamsLoading(false);
      }
    }
  }

  // Load teams on startup or if user changes and unload them if user has logged out
  useEffect(() => {
    if (user && !isLoading) {
      refreshTeams();
    } else {
      setTeams(null);
      setSelectedTeam(null);
    }
  }, [user, isLoading]);

  function selectTeam(team: Team) {
    const teamFound = teams?.find((t) => t.id == team.id);

    if (teamFound) {
      setSelectedTeam(teamFound);
    }
  }

  const contextValue: TeamContextType = {
    teams,
    selectedTeam,
    selectTeam,
    refreshTeams,
    teamsLoading,
  };

  return (
    <TeamContext.Provider value={contextValue}>{children}</TeamContext.Provider>
  );
};

export const useTeamContext = (): TeamContextType => {
  const context = useContext(TeamContext);

  if (!context) {
    throw new Error("useTeamContext must be used within a provider!");
  }

  return context;
};
