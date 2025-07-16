import { useEffect, useState } from "react";
import { useAuth } from "../contexts/AuthContext";
import { Avatar, Button, Dropdown, Space, Spin, type MenuProps } from "antd";
import { DownOutlined } from "@ant-design/icons";
import { GetTeamsForUser } from "../api/teamActions";
import type { Team } from "../types/Team";

export default function TeamSelector() {
  const { user, isLoading } = useAuth();
  const [team, setTeam] = useState<string | null>(null);
  const [dropdownItems, setDropdownItems] = useState<MenuProps["items"]>([]);
  const [teamsLoading, setTeamsLoading] = useState<boolean>(false);

  useEffect(() => {
    const fetchTeamsAndMap = async () => {
      if (user && !isLoading) {
        setTeamsLoading(true);
        const response = await GetTeamsForUser(user.tokens.accessToken);
        if (response.success) {
          const items: MenuProps["items"] = response.data.map((team: Team) => ({
            key: team.name,
            label: team.name,
            icon: (
              <Avatar style={{ backgroundColor: getColorForTeam(team.name) }}>
                {getInitials(team.name)}
              </Avatar>
            ),
          }));

          setDropdownItems(items);
          setTeamsLoading(false);
        }
      }
    };

    fetchTeamsAndMap();
  }, [user, isLoading]);

  return (
    <Dropdown
      menu={{
        items: dropdownItems,
        selectedKeys: team ? [team] : [],
        onClick: (e) => {
          setTeam(e.key);
        },
        disabled: dropdownItems!.length <= 0,
      }}
      disabled={dropdownItems!.length <= 0}
    >
      <Button style={{ width: "14rem" }}>
        {teamsLoading && <Spin size="small" />}
        {teamsLoading === false && (
          <Space>
            {team ?? "Select a Team"}
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

// Utility to generate a color from the team name (can customize)
function getColorForTeam(name: string) {
  const hash = [...name].reduce((acc, char) => acc + char.charCodeAt(0), 0);
  const colors = ["#f56a00", "#7265e6", "#ffbf00", "#00a2ae", "#52c41a"];
  return colors[hash % colors.length];
}
