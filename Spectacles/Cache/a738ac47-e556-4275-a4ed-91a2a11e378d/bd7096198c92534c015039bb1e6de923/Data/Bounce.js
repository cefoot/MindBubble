// @input float speed = 2.0
// @input float amplitude = 0.2

var startPosition = script.getTransform().getWorldPosition();

function onUpdate(eventData) {
    var time = getTime() * script.speed;
    // Use Math.abs to create a bouncing effect
    var offset = Math.abs(Math.sin(time)) * script.amplitude;
    var newPosition = startPosition.add(new vec3(0, offset, 0));
    script.getTransform().setWorldPosition(newPosition);
}

script.createEvent("UpdateEvent").bind(onUpdate);