using MessagePack;
using System.Numerics;

namespace NotEngine.ECS.Components;
[MessagePackObject(keyAsPropertyName: true)]
public partial class TransformComponent: Component
{
    private Vector3 _position = Vector3.Zero;
    private Quaternion _rotation = Quaternion.Identity;
    private Vector3 _scale = Vector3.One;

    private Matrix4x4? _parentMatrix;

    private Vector3 _worldPosition;
    private Quaternion _worldRotation;
    private Vector3 _worldScale;
    private Matrix4x4 _worldTransform;

    public Vector3 Position
    {
        get => _position;
        set
        {
            if (value != _position)
            {
                _position = value;
                UpdateWorldTransform();
            }
        }
    }

    public Quaternion Rotation
    {
        get => _rotation;
        set
        {
            if (value != _rotation)
            {
                _rotation = value;
                UpdateWorldTransform();
            }
        }
    }

    public Vector3 Scale
    {
        get => _scale;
        set
        {
            if (value != _scale)
            {
                _scale = value;
                UpdateWorldTransform();
            }
        }
    }


    [IgnoreMember]
    public Matrix4x4 Transform => Matrix4x4.CreateScale(_scale) *
                                  Matrix4x4.CreateFromQuaternion(_rotation) *
                                  Matrix4x4.CreateTranslation(_position);
    [IgnoreMember]
    public Vector3 WorldPosition
    {
        get => _worldPosition;
        set
        {
            if (value != _worldPosition)
            {
                SetWorldPosition(value);
            }
        }
    }
    [IgnoreMember]
    public Quaternion WorldRotation
    {
        get => _worldRotation;
        set
        {
            if (value != _worldRotation)
            {
                SetWorldRotation(value);
            }
        }
    }
    [IgnoreMember]
    public Vector3 WorldScale
    {
        get => _worldScale;
        set
        {
            if (value != _worldScale)
            {
                SetWorldScale(value);
            }
        }
    }
    [IgnoreMember]
    public Matrix4x4 WorldTransform
    {
        get => _worldTransform;
        set
        {
            if (value != _worldTransform)
            {
                SetWorldTransform(value);
            }
        }
    }

    public void SetWorldPosition(Vector3 worldPosition)
    {
        _worldPosition = worldPosition;

        if (_parentMatrix.HasValue)
        {
            Matrix4x4.Invert(_parentMatrix.Value, out var parentInverse);
            _position = Vector3.Transform(worldPosition, parentInverse);
        }
        else
        {
            _position = worldPosition;
        }

        UpdateWorldTransform();
    }

    public void SetWorldRotation(Quaternion worldRotation)
    {
        _worldRotation = worldRotation;

        if (_parentMatrix.HasValue)
        {
            Matrix4x4.Invert(_parentMatrix.Value, out var parentInverse);
            Matrix4x4.Decompose(parentInverse, out _, out var parentRotation, out _);
            _rotation = Quaternion.Conjugate(parentRotation) * worldRotation;
        }
        else
        {
            _rotation = worldRotation;
        }

        UpdateWorldTransform();
    }

    public void SetWorldScale(Vector3 worldScale)
    {
        _worldScale = worldScale;

        if (_parentMatrix.HasValue)
        {
            Matrix4x4.Invert(_parentMatrix.Value, out var parentInverse);
            Vector3 parentScale = Vector3.Transform(Vector3.One, parentInverse);
            _scale = Vector3.Divide(worldScale, parentScale);
        }
        else
        {
            _scale = worldScale;
        }

        UpdateWorldTransform();
    }

    public void SetWorldTransform(Matrix4x4 worldTransform)
    {
        _worldTransform = worldTransform;

        // First decompose the world transform matrix
        Matrix4x4.Decompose(_worldTransform, out _worldScale, out _worldRotation, out _worldPosition);

        if (_parentMatrix.HasValue)
        {
            Matrix4x4.Invert(_parentMatrix.Value, out var parentInverse);
            Matrix4x4 localTransform = worldTransform * parentInverse;

            // Decompose the local transform
            Matrix4x4.Decompose(localTransform, out _scale, out _rotation, out _position);
        }
        else
        {
            // If there's no parent matrix, assign world transform directly to local transform
            _position = _worldPosition;
            _scale = _worldScale;
            _rotation = _worldRotation;
        }
    }

    internal void SetParentTransformMatrix(Matrix4x4? parentTransform)
    {
        if (parentTransform != _parentMatrix)
        {
            _parentMatrix = parentTransform;
            UpdateWorldTransform();
        }
    }

    private void UpdateWorldTransform()
    {
        _worldTransform = _parentMatrix.HasValue ? _parentMatrix.Value * Transform : Transform;
        Matrix4x4.Decompose(_worldTransform, out _worldScale, out _worldRotation, out _worldPosition);
    }
}