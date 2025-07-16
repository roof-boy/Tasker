import { Button, Result } from "antd";
import { useNavigate } from "react-router";

export default function ServerFail() {
  const navigate = useNavigate();
  return (
    <div className="w-screen h-screen flex items-center justify-center">
      <Result
        status="500"
        title="500 - Server Error"
        subTitle="Sorry, we can't handle this request at the moment"
        extra={
          <Button onClick={() => navigate("/")} type="primary">
            Back Home
          </Button>
        }
      />
    </div>
  );
}
