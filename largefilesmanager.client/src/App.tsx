import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import { GenerateFile } from "./components/GenerateFile";
import { SortFile } from "./components/SortFile";

const App = () => {
  return (
    <Router>
      <div>
      <nav>
          <ul style={{ display: "flex", listStyle: "none", padding: 0 }}>
            <li style={{ marginRight: "20px" }}>
              <Link to="/">Home</Link>
            </li>
            <li style={{ marginRight: "20px" }}>
              <Link to="/generate-file">Generate File</Link>
            </li>
            <li style={{ marginRight: "20px" }}>
              <Link to="/sort-file">Sort File</Link>
            </li>
          </ul>
        </nav>

        <Routes>
          <Route
            path="/"
            element={
              <div>
                <h2>Welcome to the File Management App</h2>
                <p>Use the menu to navigate to file generation.</p>
              </div>
            }
          />
          <Route path="/generate-file" element={<GenerateFile />} />
          <Route path="/sort-file" element={<SortFile/>} />
        </Routes>
      </div>
    </Router>
  );
};

export default App;
