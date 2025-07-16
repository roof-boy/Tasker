import { Alert, Button, Card, Form, Input } from "antd";
import { useEffect, useRef, useState } from "react";
import type { FormInstance } from "antd/es/form";
import type { ReactNode } from "react";
import LoginUser from "../../api/loginUser";
import { useAuth } from "../../contexts/AuthContext";
import { useNavigate } from "react-router";

type LoginFormType = {
  username?: string;
  password?: string;
};

export default function Login() {
  const [isLoginLoading, setLoginLoading] = useState<boolean>(false);
  const [errorMessage, setErrorMessage] = useState("");
  const formRef = useRef<FormInstance<LoginFormType>>(null);

  const { login, isLoading } = useAuth();
  const navigate = useNavigate();

  const cardActions: ReactNode[] = [
    <Button
      htmlType="button"
      type="text"
      style={{ width: "100%" }}
      loading={isLoginLoading}
      onClick={() => formRef.current?.submit()}
    >
      Login
    </Button>,
  ];

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Enter") {
        formRef.current?.submit();
      }
    };

    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, []);

  return (
    <Card
      title="Login to Tasker"
      extra={
        <Button type="link" style={{ padding: 0 }}>
          Or Register
        </Button>
      }
      variant="borderless"
      actions={cardActions}
      className="min-w-86"
      styles={{
        actions: { padding: "0px 15px" },
        body: { padding: "20px 15px" },
      }}
    >
      {errorMessage && (
        <Alert
          type="error"
          message={errorMessage}
          style={{ marginBottom: "15px" }}
        />
      )}
      <Form
        name="login"
        layout="vertical"
        ref={formRef}
        disabled={isLoginLoading}
        onFinish={async (values) => {
          setLoginLoading(true);
          var response = await LoginUser(values);
          if (response.success && !isLoading) {
            login(response.data);
            navigate("/");
          } else if (!response.success) {
            setErrorMessage(response.error);
          }
          setLoginLoading(false);
        }}
      >
        <Form.Item<LoginFormType>
          label="Username"
          name="username"
          rules={[{ required: true, message: "Please input your username!" }]}
        >
          <Input />
        </Form.Item>
        <Form.Item<LoginFormType>
          label="Password"
          name="password"
          rules={[{ required: true, message: "Please input your Password!" }]}
          style={{ marginBottom: 0 }}
        >
          <Input.Password />
        </Form.Item>
      </Form>
    </Card>
  );
}
