import React from "react";
import { BookOutlined, BugOutlined, HomeOutlined } from "@ant-design/icons";
import type { MenuProps } from "antd";
import { Button, Flex, Layout, Menu, theme } from "antd";
import { Outlet, useLocation, useNavigate } from "react-router";
import { useAuth } from "../contexts/AuthContext";
import TeamSelector from "../components/TeamSelector";

const { Header, Content, Footer, Sider } = Layout;

type MenuItem = Required<MenuProps>["items"][number];

const items: MenuItem[] = [
  {
    key: "/",
    label: "Dashboard",
    icon: <HomeOutlined />,
  },
  {
    key: "/projects",
    label: "Projects",
    icon: <BookOutlined />,
  },
  {
    key: "/issues",
    label: "Issues",
    icon: <BugOutlined />,
  },
];
const App: React.FC = () => {
  const {
    token: { colorBgContainer },
  } = theme.useToken();

  const navigate = useNavigate();
  const page = useLocation();

  const { logout } = useAuth();

  return (
    <Layout style={{ minHeight: "100vh" }}>
      <Header
        style={{
          background: colorBgContainer,
          borderBottom: "1px solid var(--color-gray-200)",
          padding: "0px 1rem",
        }}
      >
        <TeamSelector />
      </Header>
      <Layout>
        <Sider
          breakpoint="lg"
          width={"16rem"}
          theme="light"
          style={{ borderInlineEnd: "1px solid var(--color-gray-200)" }}
        >
          <Flex style={{ minHeight: "100%" }} vertical justify="space-between">
            <Menu
              theme="light"
              selectedKeys={[page.pathname]}
              mode="inline"
              items={items}
              onClick={(e) => {
                navigate(e.key);
              }}
              style={{ borderInlineEnd: 0 }}
            />
            <Button style={{ margin: "1rem 1rem" }} onClick={() => logout()}>
              Logout
            </Button>
          </Flex>
        </Sider>
        <Layout>
          <Content style={{ padding: "2rem" }}>
            <Outlet />
          </Content>
          <Footer style={{ textAlign: "center" }}>
            Tasker ©{new Date().getFullYear()} - Created by Elias Sovatzis
          </Footer>
        </Layout>
      </Layout>
    </Layout>
  );
};

export default App;
