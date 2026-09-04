import { Navbar, Nav, Container, Dropdown } from "react-bootstrap";
import { Link } from "react-router-dom"
import { useAuth } from "../../context/AuthContext";
import { useNavigate } from "react-router-dom"
export default function Header() {

    const { user, logout } = useAuth();
    const navigate = useNavigate();

    function logoutFnc ()
    {
        logout();
        navigate("/");
    };

    return (

        <Navbar
            bg="dark"
            variant="dark"
            expand="lg"
            sticky="top"
            className="shadow"
        >

            <Container>

                <Navbar.Brand as={Link} to="/">
                    ASPOTUS
                </Navbar.Brand>

                <Navbar.Toggle />

                <Navbar.Collapse>

                    <Nav className="ms-auto">

                        <Nav.Link as={Link} to="/">
                            Главная
                        </Nav.Link>

                        <Nav.Link as={Link} to="/cars">
                            Авто
                        </Nav.Link>

                        <Nav.Link as={Link} to="/parts">
                            Запчасти
                        </Nav.Link>

                        <Nav.Link as={Link} to="/about">
                            О нас
                        </Nav.Link>

                        {user ? (
                            <Dropdown>
                                <Dropdown.Toggle>
                                    {user.fullName || user.name}
                                </Dropdown.Toggle>

                                <Dropdown.Menu>
                                    <Dropdown.Item as={Link} to="/profile" style={{ textDecoration: 'none', color: 'inherit' }}>
                                        Профиль
                                    </Dropdown.Item>
                                    <Dropdown.Item as={Link} to="/cart" style={{ textDecoration: 'none', color: 'inherit' }}>
                                        Корзина
                                    </Dropdown.Item>
                                    <Dropdown.Item onClick={logoutFnc}>
                                        Выйти
                                    </Dropdown.Item>
                                </Dropdown.Menu>
                            </Dropdown>
                        ) : (
                            <>
                                <Nav.Link as={Link} to="/login">Профиль</Nav.Link>
                            </>
                        )}

                    </Nav>

                </Navbar.Collapse>

            </Container>

        </Navbar>

    )

}