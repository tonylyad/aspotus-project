import { Row, Col } from "react-bootstrap"
import PartCard from "./PartCard"

export default function PartList({ parts }) {

    return (

        <Row>

            {parts?.map(part => (

                <Col
                    lg={4}
                    md={6}
                    sm={12}
                    className="mb-4"
                    key={part.id}
                >

                    <PartCard part={part} />

                </Col>

            ))}

        </Row>

    )

}