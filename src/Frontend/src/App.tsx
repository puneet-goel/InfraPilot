import { useEffect, useState } from "react";

function App() {
  const [message, setMessage] = useState("");

  useEffect(() => {
    fetch("/api/health")
      .then(res => res.json())
      .then(data => {
        setMessage(data.status);
      });
  }, []);

  return (
    <div
      style={{
        padding: "40px",
        fontFamily: "Arial"
      }}
    >
      <h1>InfraPilot UI</h1>

      <p>{message}</p>
    </div>
  );
}

export default App;