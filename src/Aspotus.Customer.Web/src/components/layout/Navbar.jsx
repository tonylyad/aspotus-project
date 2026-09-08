import { Navbar, Nav, Container, Dropdown } from "react-bootstrap";
import { Link } from "react-router-dom"
import { useAuth } from "../../context/AuthContext";
import { useCart } from "../../context/CartContext";
import { useNavigate } from "react-router-dom"
import { FiShoppingCart } from "react-icons/fi";
export default function Header() {

    const { user, logout } = useAuth();
    const { cart } = useCart();
    const navigate = useNavigate();
    const cartItemCount = cart.reduce((total, item) => total + Number(item.quantity || 0), 0);

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

                        <Nav.Link
                            as={Link}
                            to="/cart"
                            className="header-cart-link"
                            aria-label={`Корзина: ${cartItemCount} товаров`}
                            title="Корзина"
                        >
                            <FiShoppingCart aria-hidden="true" />
                            <span className="header-cart-count">{cartItemCount}</span>
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
