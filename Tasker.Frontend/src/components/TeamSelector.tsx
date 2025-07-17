import { useEffect, useMemo } from "react";
import { useTeamContext } from "../contexts/TeamContext";
import { Avatar, Button, Dropdown, Space, Spin, type MenuProps } from "antd";
import { DownOutlined } from "@ant-design/icons";
import type { Team } from "../types/Team";

export default function TeamSelector() {
  const { teams, selectedTeam, selectTeam, teamsLoading, refreshTeams } =
    useTeamContext();

  // Map teams to dropdown items
  const dropdownItems = useMemo<MenuProps["items"]>(() => {
    if (!teams) return [];
    return teams.map((team: Team) => ({
      key: team.id,
      label: team.name,
      icon: (
        <Avatar style={{ backgroundColor: getColorForTeam(team.name) }}>
          {getInitials(team.name)}
        </Avatar>
      ),
    }));
  }, [teams]);

  useEffect(() => {
    refreshTeams(); // in case not loaded (safe if already loaded)
  }, []);

  return (
    <Dropdown
      menu={{
        items: dropdownItems,
        selectedKeys: selectedTeam ? [selectedTeam.id] : [],
        onClick: (e) => {
          const team = teams?.find((t) => t.id === e.key);
          if (team) selectTeam(team);
        },
        disabled: !teams || teams.length === 0,
      }}
      disabled={!teams || teams.length === 0}
    >
      <Button style={{ width: "14rem" }}>
        {teamsLoading && <Spin size="small" />}
        {!teamsLoading && (
          <Space>
            {selectedTeam?.name ?? "Select a Team"}
            <DownOutlined />
          </Space>
        )}
      </Button>
    </Dropdown>
  );
}

// Utility to get initials like "JC" from "Jira Copiers"
function getInitials(name: string) {
  return name
    .split(" ")
    .map((word) => word[0])
    .join("")
    .toUpperCase();
}

// Utility to generate a color from the team name
function getColorForTeam(name: string) {
  const hash = [...name].reduce((acc, char) => acc + char.charCodeAt(0), 0);
  const colors = ["#f56a00", "#7265e6", "#ffbf00", "#00a2ae", "#52c41a"];
  return colors[hash % colors.length];
}